COLOR='\033[0;36m'
NC='\033[0m' # No Color

echo " "

echo "${COLOR}Pushing build for OSX${NC}"
butler push Builds/osx anttihaavikko/mildred:osx --fix-permissions

echo "${COLOR}Pushing build for Windows${NC}"
butler push Builds/win anttihaavikko/mildred:win --fix-permissions

echo "${COLOR}Pushing build for Linux${NC}"
butler push Builds/linux anttihaavikko/mildred:linux

echo "${COLOR}Pushing build for HTML5${NC}"
butler push Builds/html5 anttihaavikko/mildred:html5