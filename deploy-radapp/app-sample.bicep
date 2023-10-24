import radius as radius

resource env 'Applications.Core/environments@2023-10-01-preview' = {
  name: 'jjenv'
  properties: {
    compute: {
      kind: 'kubernetes'   // What kind of container runtime to use
      namespace: 'jjenv' // Where application resources are rendered
    }
  }
}

resource app 'Applications.Core/applications@2023-10-01-preview' = {
  name: 'jjapp'
  properties: {
    environment: env.id
  }
}

resource demo 'Applications.Core/containers@2023-10-01-preview' = {
  name: 'jjdemo'
  properties: {
    application: app.id
    container: {
      image: 'ghcr.io/radius-project/tutorial/webapp:edge'
      ports: {
        web: {
          containerPort: 3000
        }
      }
    }
  }
}
