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
    return this.http.get(`${this.apiUrl}/GetRecords?currentPage=${currentPage}&itemsPerPage=${itemsPerPage}`);
  }
}
