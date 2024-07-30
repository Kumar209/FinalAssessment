import { HttpClient, HttpParams } from '@angular/common/http';
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

    // const headers = new HttpHeaders({
    //   'Authorization': `Bearer kdsjfldsjflkjsdlkfjlksdjflksdjlkfjsdlkfjlksdjflkjsdlfjklsdjfklsdlkfjlkdsjflkdsjflkjsdlkfjsaldkfjlsd` 
    // });

    // return this.http.get<any>(`${this.apiUrl}/GetRecords`, { params, headers });
    
    return this.http.get<any>(`${this.apiUrl}/GetRecords`, { params });
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


  // downloadExcel() {
  //   const headers = new HttpHeaders({
  //     'Authorization': `Bearer ${this.getToken()}` // Adjust token retrieval as needed
  //   });

  //   return this.http.get(`${this.apiUrl}/User/DownloadExcel`, {
  //     headers,
  //     responseType: 'blob' // Important to handle binary data
  //   });
  // }


  // downloadExcel() {
  //   this.service.downloadExcel().subscribe({
  //     next: (response) => {
  //       const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
  //       saveAs(blob, 'Users.xlsx');
  //     },
  //     error: (err) => {
  //       this.toastr.error(err.error?.message || 'Something went wrong', 'Error!');
  //     }
  //   });
  // }
}
