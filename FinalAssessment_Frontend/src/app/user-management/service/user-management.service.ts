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

    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);

    
    return this.http.get<any>(`${this.apiUrl}/GetRecords`, { params , headers : head_obj });
  }


  addUser(userDetails : any) : Observable<any>{

    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);

    return this.http.post<any>(`${this.apiUrl}/AddUser`, userDetails, { headers : head_obj });
  }


  deleteUser(id : number) : Observable<any>{

    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);

    
    return this.http.delete<any>(`${this.apiUrl}/RemoveUser/${id}` , { headers : head_obj });
  }

  downloadExcel() {

    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);


    return this.http.get(`${this.apiUrl}/DownloadExcel`, {
      headers  : head_obj,
      responseType: 'blob'
    });
  }


  getUserById(id : number) {
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);
    console.log(id);

    return this.http.get(`${this.apiUrl}/GetUserById/${id}`, {headers : head_obj});
  }


  updateUser(userDetails : any) {
    let head_obj = new HttpHeaders().set("Authorization", "Bearer "+this.token);

    return this.http.put(`${this.apiUrl}/UpdateUser`, userDetails, {headers : head_obj});
  }
}
