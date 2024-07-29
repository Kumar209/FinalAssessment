import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  constructor(private http : HttpClient) { }

  private apiUrl = 'https://localhost:44320/api/User';


  getRecordPerPage(currentPage : number, itemsPerPage : number) : Observable<any>{
    return this.http.get<any[]>(`${this.apiUrl}/GetRecords?currentPage=${currentPage}&itemsPerPage=${itemsPerPage}`);
  }


  addUser(userDetails : any) : Observable<any>{
    return this.http.post<any>(`${this.apiUrl}/AddUser`, userDetails);
  }


  deleteUser(id : number) : Observable<any>{
    return this.http.delete<any>(`${this.apiUrl}/RemoveUser/${id}`);
  }

  getActiveUserCount() : Observable<any>{
    return this.http.get<any>(`${this.apiUrl}/ActiveRecords`);
  }

}
