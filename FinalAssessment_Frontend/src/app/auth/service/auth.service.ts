import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserCrendentail } from '../Interface/IUserCredential';
import { map, Observable } from 'rxjs';
import { IResetCredential } from '../Interface/IResetCredential';
import { ActivatedRoute } from '@angular/router';
import { IChangeCredential } from '../Interface/IChangeCredential';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isAuthenticated = false;
  public authSecretKey = 'BearerToken';
  public authUserCookieKey = 'UserDetails';

  baseUrl : string = "https://localhost:44320/api/Account";


  constructor(private http : HttpClient, private activatedRoute: ActivatedRoute) { 
    //Setting this value to true and false if token key is present or not in local storage
    this.isAuthenticated = !!localStorage.getItem(this.authSecretKey);
  }


  isAuthenticatedUser(): boolean {
    return this.isAuthenticated;
  }



  // login(userCredential : IUserCrendentail) : Observable<any>{
  //   return this.http.post<any>(`${this.baseUrl}/LoginUser` , userCredential)
  //   .pipe(
  //     map(response => {
  //       localStorage.setItem(this.authSecretKey, response.token);
  //       localStorage.setItem(this.authUserCookieKey, JSON.stringify(response.requiredDataForFrontend));
  //       return response;
  //     })
  //   );
  // }

  login(userCredential : IUserCrendentail) : Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/LoginUser` , userCredential)
  }

  
  logout(): void {
    localStorage.removeItem(this.authSecretKey);
    localStorage.removeItem("UserDetails");
    this.isAuthenticated = false;
  }


  forgotPassword(email : string) : Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/ForgotPassword?email=${email}`, {});
  }


  resetPassword(resetCredential : IResetCredential) : Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ResetPassword`, resetCredential);
  }


  changePassword(changePasswordCredential : IChangeCredential) : Observable<any> {    
    return this.http.post<any>(`${this.baseUrl}/ChangePassword`, changePasswordCredential);
  }


  activateAccount() : Observable<any>{
    return this.http.patch<any>(`${this.baseUrl}/ActivateUser`, {});
  }
}
