import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';
import { Observable, mergeMap, tap } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: MsalService){}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    return this.authService.acquireTokenSilent({
        scopes: ["openid"]
      })
      .pipe(
        mergeMap((response: AuthenticationResult) => {
            const accessToken = response.accessToken;
            req = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${accessToken}`
                }
              });
            return next.handle(req);
          })
      );

  }
}