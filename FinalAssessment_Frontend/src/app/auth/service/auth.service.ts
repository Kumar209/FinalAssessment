import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserCrendentail } from '../Interface/IUserCredential';
import { Observable } from 'rxjs';
import { IResetCredential } from '../Interface/IResetCredential';
import { ActivatedRoute } from '@angular/router';
import { IChangeCredential } from '../Interface/IChangeCredential';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  routeToken : string;

  token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTdWJqZWN0IiwianRpIjoiZDVmNWQ5MmUtNWQ1Ni00NDM5LTg5NzItM2NlNTlmN2FhZDAzIiwiSWQiOiIxIiwiRW1haWwiOiJwcmFzaGFudGt1bWFybG1wNjY2QGdtYWlsLmNvbSIsImV4cCI6MTcyMjA3NjAxNCwiaXNzIjoiSnd0SXNzdWVyIiwiYXVkIjoiSnd0QXVkaWVuY2UifQ.hz0_fM3Mnyjy79abFoesofIDUK3jRP45NhLd4hVZ97o";

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


  changePassword(changePasswordCredential : IChangeCredential) : Observable<any> {
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);
    return this.http.post<any>(`${this.baseUrl}/ChangePassword`, changePasswordCredential, {headers : head_obj});
  }
}
