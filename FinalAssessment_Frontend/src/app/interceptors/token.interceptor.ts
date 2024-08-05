import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from '../auth/service/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService, private router: Router, private activatedRoute: ActivatedRoute, private toastr : ToastrService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let authToken;


    //If login api then simply return
    if (request.url.includes('/LoginUser')) {
      return next.handle(request);
    }



    //If reset password and activate user api then we fetch token from params
    if(request.url.includes('/ResetPassword') || request.url.includes('/ActivateUser')){
      const routeToken =  this.activatedRoute.snapshot.queryParams['token'];

      if(!routeToken){
        this.toastr.error('Invalid token');
        this.router.navigate(['auth/login']);
        return throwError(() => new Error('Route Token is missing.'));
      }

      authToken = routeToken;
    }
    else {
      authToken = localStorage.getItem(this.authService.authSecretKey);
    }



    if (authToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${authToken}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.toastr.warning('Session expired. Please login again!');

          localStorage.removeItem(this.authService.authSecretKey);
          localStorage.removeItem(this.authService.authUserCookieKey);

          this.router.navigate(['auth/login']);
        }

        return throwError(() => new Error(error.message));
      })
    );
  }
}
