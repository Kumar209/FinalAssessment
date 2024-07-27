import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserCrendentail } from '../Interface/IUserCredential';
import { Observable } from 'rxjs';
import { IResetCredential } from '../Interface/IResetCredential';
import { ActivatedRoute } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  routeToken : string;

  constructor(private http : HttpClient, private activatedRoute: ActivatedRoute) { 
    this.routeToken = this.activatedRoute.snapshot.queryParams['token']
  }

  baseUrl : string = "https://localhost:44320/api/Account";


  login(userCredential : IUserCrendentail) : Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/LoginUser` , userCredential);
  }


  forgotPassword(email : string) : Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/ForgotPassword?email=${email}`, {});
  }


  resetPassword(resetCredential : IResetCredential) : Observable<any> {
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.routeToken);
    return this.http.post<any>(`${this.baseUrl}/ResetPassword`, resetCredential, {headers : head_obj});
  }
}
