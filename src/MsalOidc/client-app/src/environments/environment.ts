export const environment = {
    production: false,
    msalConfig: {
        auth: {
            clientId: '3b810eae-5d6d-4aef-86f6-8c713a156561',
            authority: 'https://login.microsoftonline.com/6034845f-1372-457d-8d8e-3ea438724be1'
        }
    },
    apiConfig: {
        scopes: ['oidc'],
        uri: 'https://localhost:5114'
    }
  };