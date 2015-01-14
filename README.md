MonoWebPublisher
================

a small tool for publishing(repackaging) ASP.NET web projects in mono(linux)

Inspired by this thread http://stackoverflow.com/questions/14296517/mono-xbuild-publish-mvc-site

Here is a sample script for building a ASP.NET MVC 4 project and pushing to deploy repository with gitlab in linux environment.
```bash
(
git submodule update --init
xbuild SandboxMVC4.sln /p:Configuration=Release
mkdir ../../deploy/$CI_BUILD_ID -p
wget -O ../../deploy/$CI_BUILD_ID/MonoWebPublisher.exe https://github.com/z-ji/MonoWebPublisher/releases/download/v0.1/MonoWebPublisher.exe
echo 'ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no $*' > ../../deploy/$CI_BUILD_ID/ssh
chmod +x ../../deploy/$CI_BUILD_ID/ssh
GIT_TRACE=1 GIT_SSH=../../deploy/$CI_BUILD_ID/ssh git clone git@repo.z7.org.uk:zji/sandboxmvc4-deploy.git ../../deploy/$CI_BUILD_ID/sandboxmvc4-deploy
mono ../../deploy/$CI_BUILD_ID/MonoWebPublisher.exe SandboxMVC4/SandboxMVC4.csproj ../../deploy/$CI_BUILD_ID/sandboxmvc4-deploy
cd ../../deploy/$CI_BUILD_ID/sandboxmvc4-deploy
git add .
git diff-index --quiet HEAD || git commit -a -m "build:$CI_BUILD_ID"
GIT_TRACE=1 GIT_SSH=../ssh git push -u origin master
)
rm ../../deploy/$CI_BUILD_ID -rf
```
