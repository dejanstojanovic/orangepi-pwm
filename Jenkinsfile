pipeline {
  agent any
  stages {
    stage('build') {
      steps {
        dotnetRestore(project: 'OrangePi.Display.Status.Service.csproj', runtime: 'net8.0')
      }
    }

  }
}