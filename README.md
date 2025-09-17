Testing if this push triggers build_and_test workflow again

Install CHIRP cli locally on your computer guide
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release Chirp.CLI

//if already installed then uninstall old one
dotnet tool uninstall --global Chirp.CLI

Add dotnet tools to your PATH
Tools install into ~/.dotnet/tools. Add this line to your ~/.zshrc (or ~/.bashrc if using bash):
export PATH="$PATH:$HOME/.dotnet/tools"
source ~/.zshrc
