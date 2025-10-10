#!/usr/bin/env bash
dotnet test -c Release -p:PublishAot=false -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml && \
  reportgenerator.exe "-reports:bin/Release/**/*.cobertura.xml" "-targetdir:./coverage"