import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  constructor(private http : HttpClient) { }

  private apiUrl = 'https://localhost:44320/api/User';

  private token = localStorage.getItem('BearerToken');


  getRecordPerPage(currentPage: number, itemsPerPage: number, status: string | null, sortBy: string | null, isAscending: boolean) : Observable<any>{

    const params = new HttpParams()
    .set('currentPage', currentPage.toString())
    .set('itemsPerPage', itemsPerPage.toString())
    .set('status', status || '')
    .set('sortBy', sortBy || '')
    .set('isAscending', isAscending.toString());

    // const headers = new HttpHeaders({
    //   'Authorization': "Bearer "+this.token
    // });

    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);

    
    return this.http.get<any>(`${this.apiUrl}/GetRecords`, { params , headers : head_obj });
  }


  addUser(userDetails : any) : Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}` 
    });


    return this.http.post<any>(`${this.apiUrl}/AddUser`, userDetails, { headers });
  }


  deleteUser(id : number) : Observable<any>{
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}` 
    });

    
    return this.http.delete<any>(`${this.apiUrl}/RemoveUser/${id}` , { headers });
  }

  downloadExcel() {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}` 
    });


    return this.http.get(`${this.apiUrl}/DownloadExcel`, {
      headers,
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
