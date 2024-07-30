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
  routeToken : string;

  private isAuthenticated = false;
  public authSecretKey = 'BearerToken';

  public token;

  baseUrl : string = "https://localhost:44320/api/Account";


  constructor(private http : HttpClient, private activatedRoute: ActivatedRoute) { 
    //Setting this value to true and false if token key is present or not in local storage
    this.isAuthenticated = !!localStorage.getItem(this.authSecretKey);

    this.token = localStorage.getItem(this.authSecretKey);

    //Getting the token from query params
    this.routeToken = this.activatedRoute.snapshot.queryParams['token']
  }


  isAuthenticatedUser(): boolean {
    return this.isAuthenticated;
  }



  login(userCredential : IUserCrendentail) : Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/LoginUser` , userCredential)
    .pipe(
      map(response => {
        localStorage.setItem(this.authSecretKey, response.token);
        localStorage.setItem("UserDetails", JSON.stringify(response.requiredDataForFrontend));
        return response;
      })
    );
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
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.routeToken);
    
    return this.http.post<any>(`${this.baseUrl}/ResetPassword`, resetCredential, {headers : head_obj});
  }


  changePassword(changePasswordCredential : IChangeCredential) : Observable<any> {
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);
    
    return this.http.post<any>(`${this.baseUrl}/ChangePassword`, changePasswordCredential, {headers : head_obj});
  }


  activateAccount() : Observable<any>{
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.routeToken);

    return this.http.patch<any>(`${this.baseUrl}/ActivateUser`, {}, {headers : head_obj});
  }
}
