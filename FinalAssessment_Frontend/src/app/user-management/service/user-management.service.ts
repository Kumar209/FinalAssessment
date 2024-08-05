import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  constructor(private http : HttpClient) { }

  private apiUrl = 'https://localhost:44320/api/User';


  getRecordPerPage(currentPage: number, itemsPerPage: number, status: string | null, sortBy: string | null, isAscending: boolean) : Observable<any>{

    const params = new HttpParams()
    .set('currentPage', currentPage.toString())
    .set('itemsPerPage', itemsPerPage.toString())
    .set('status', status || '')
    .set('sortBy', sortBy || '')
    .set('isAscending', isAscending.toString());

    
    return this.http.get<any>(`${this.apiUrl}/GetRecords`, { params });
  }


  addUser(userDetails : any) : Observable<any>{
    return this.http.post<any>(`${this.apiUrl}/AddUser`, userDetails);
  }


  deleteUser(id : number) : Observable<any>{
    return this.http.delete<any>(`${this.apiUrl}/RemoveUser/${id}`);
  }


  downloadExcel() {
    return this.http.get(`${this.apiUrl}/DownloadExcel`, {
      responseType: 'blob'
    });
  }


  getUserById(id : number) {
    return this.http.get(`${this.apiUrl}/GetUserById/${id}`);
  }


  updateUser(userDetails : any) {
    return this.http.put(`${this.apiUrl}/UpdateUser`, userDetails);
  }
}
