ARG VERSION=0.157.0
# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT=5.0
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:${VERSION}-${VARIANT}

# [Optional] Uncomment this section to install additional OS packages.
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    # Provide dependencies for gui application
    && apt-get -y install xorg
