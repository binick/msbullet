#!/usr/bin/env bash

# Stop script if unbound variable found (use ${var:-} if intentional)
set -u

# Stop script if command returns non-zero exit code.
# Prevents hidden errors caused by missing error code propagation.
set -e

usage()
{
  echo "Common settings:"
  echo "  --configuration <value>    Build configuration: 'Debug' or 'Release' (short: -c)"
  echo "  --platform <value>         Platform configuration: 'x86', 'x64' or any valid Platform value to pass to msbuild"
  echo "  --verbosity <value>        Msbuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
  echo "  --binaryLog                Create MSBuild binary log (short: -bl)"
  echo "  --help                     Print help and exit (short: -h)"
  echo ""

  echo "Actions:"
  echo "  --restore                  Restore dependencies (short: -r)"
  echo "  --build                    Build solution (short: -b)"
  echo "  --rebuild                  Rebuild solution"
  echo "  --test                     Run all unit tests in the solution (short: -t)"
  echo "  --integrationTest          Run all integration tests in the solution"
  echo "  --performanceTest          Run all performance tests in the solution [WIP]"
  echo "  --collect                  Collect code coverage metrics for all unit tests in the solution"
  echo "  --pack                     Package build outputs into NuGet packages"
  echo "  --sign                     Sign build outputs [WIP]"
  echo "  --clean                    Clean the solution"
  echo ""

  echo "Advanced settings:"
  echo "  --ci                     Set when running on CI server"
  echo "  --prepareMachine         Prepare machine for CI run, clean up processes after build"
  echo "  --warnAsError <value>    Sets warnaserror msbuild parameter ('true' or 'false')"
  echo ""
  
  echo "Command line arguments not listed above are passed thru to msbuild."
  echo "Arguments can also be passed in with a single hyphen."
}

source="${BASH_SOURCE[0]}"

# resolve $source until the file is no longer a symlink
while [[ -h "$source" ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"
  # if $source was a relative symlink, we need to resolve it relative to the path where the
  # symlink file was located
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"

restore=false
build=false
rebuild=false
test=false
integration_test=false
pack=false
ci=false
clean=false
collect=false

warn_as_error=true
node_reuse=true
binary_log=false
exclude_ci_binary_log=false
pipelines_log=false

projects=''
configuration='Debug'
prepare_machine=false
verbosity='minimal'
platform=''

properties=''

while [[ $# > 0 ]]; do
  opt="$(echo "${1/#--/-}" | awk '{print tolower($0)}')"
  case "$opt" in
    -help|-h)
      usage
      exit 0
      ;;
    -clean)
      clean=true
      ;;
    -configuration|-c)
      configuration=$2
      shift
      ;;
    -verbosity|-v)
      verbosity=$2
      shift
      ;;
    -platform)
      platform=$2
      shift
      ;;
    -binarylog|-bl)
      binary_log=true
      ;;
    -restore|-r)
      restore=true
      ;;
    -build|-b)
      build=true
      ;;
    -rebuild)
      rebuild=true
      ;;
    -pack)
      pack=true
      ;;
    -test|-t)
      test=true
      ;;
    -integrationtest)
      integration_test=true
      ;;
    -preparemachine)
      prepare_machine=true
      ;;
    -ci)
      ci=true
      ;;
    -warnaserror)
      warn_as_error=$2
      shift
      ;;
    -nodereuse)
      node_reuse=$2
      shift
      ;;
    *)
      properties="$properties $1"
      ;;
  esac

  shift
done

if [[ "$ci" == true ]]; then
  node_reuse=false
  binary_log=true
fi

. "$scriptroot/tools.sh"

function Build {
  local bl=""
  if [[ "$binary_log" == true ]]; then
    bl="/bl:\"$log_dir/Build.binlog\""
  fi

  local parameters=()
  if [[ "$platform" != "" ]]; then
    parameters+=("/p:Platform=$platform")
  fi
  if [[ "$configuration" != "" ]]; then
    parameters+=("/p:Configuration=$configuration")
  fi

  parameterArgs=''
  for parameter in "${parameters[@]}"
  do
    parameterArgs+="$parameter "
  done

  local targets=()
  if [[ "$restore" == true ]]; then
    targets+=("Restore")
  fi
  if [[ "$build" == true ]]; then
    targets+=("Build")
  fi
  if [[ "$rebuild" == true ]]; then
    targets+=("Rebuild")
  fi
  if [[ "$test" == true ]]; then
    targets+=("Test")
  fi
  if [[ "$integration_test" == true ]]; then
    targets+=("IntegrationTest")
  fi
  if [[ "$collect" == true ]]; then
    targets+=("CollectCoverage")
  fi
  if [[ "$pack" == true ]]; then
    targets+=("Pack")
  fi

  targetArgs=''
  for target in "${targets[@]}"
  do
    targetArgs+="/t:$target "
  done

  MSBuild \
    $bl \
    $parameterArgs \
    $targetArgs \
    $properties

  ExitWithExitCode 0
}

if [[ "$clean" == true ]]; then
  if [ -d "$artifacts_dir" ]; then
    rm -rf $artifacts_dir
    echo "Artifacts directory deleted."
  fi
  exit 0
fi

Build
