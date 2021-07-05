# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT=5.0
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# [Optional] Uncomment this section to install additional OS packages.
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    # Provide dependencies for gui application
    && apt-get -y install xorg

# We have to create this folder by ourselfes, otherwise the permissions are wrong
RUN sudo -u vscode mkdir -p /home/vscode/.local/share
