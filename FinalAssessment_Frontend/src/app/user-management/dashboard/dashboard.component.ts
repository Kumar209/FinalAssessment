import { Component, OnInit } from '@angular/core';
import { UserManagementService } from '../service/user-management.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  
  //Array to hold user records
  userData: any[] = [];

  //Total number of record which are active that isActive = 1 --Fetched from APi
  activeUserCount: number = 0;

  //Total number of inactive record present --Calculated from totalUser - activeUserCount
  inActiveUserCount: number = 0;

  //Tatal number of record present 
  totalUser: number = 0;

  //Used to track current page number
  currentPage: number = 1;

  //used to set how many organizaton should be shown on a single page of table
  itemsPerPage: number = 5;


  //Total number of pages according to itemsPerPage and records present in db
  pagedNumber: number = 0;


  // Status for filtering
  filterStatus: string | null = null;


  //Which coloumn according to which it will be sorted on click
  sortBy: string | null = null;
  
  
  isAscending: boolean = true;


  userIdToDelete! : number;



  constructor(
    private service: UserManagementService,
    private router: Router,
    private toastr: ToastrService
  ) {
  }


  ngOnInit(): void {
    this.getRecordsPerPage(this.currentPage, this.itemsPerPage, this.filterStatus, this.sortBy, this.isAscending);
  }


 
  getAddressType1(user: any): any {
    return user.prashantDbAddresses.find(
      (address : any) => address.addressTypeId === 1
    );
  }


  getAddressesType2(user: any): any {
    return user.prashantDbAddresses.filter(
      (address : any) => address.addressTypeId === 2
    );
  }



  onFilterActive() : void {
    this.filterStatus = "active";
    this.getRecordsPerPage(1, this.itemsPerPage , this.filterStatus, this.sortBy, this.isAscending);
  }

  onFilterInActive() : void {
    this.filterStatus = "inactive";
    this.getRecordsPerPage(1, this.itemsPerPage , this.filterStatus, this.sortBy, this.isAscending);
  }



  //Method used to update the current page when clicked
  onPageChange(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.pagedNumber) return; // Prevent invalid page numbers

    this.currentPage = pageNumber;

    //Here fetching record for the new page when clicked
    this.getRecordsPerPage(this.currentPage, this.itemsPerPage, this.filterStatus, this.sortBy, this.isAscending); 
  }


  // Basically generating array so through that we can loop through our li tag in pagination
  totalPages(): number[] {
    const pages: number[] = [];

    for (let i = 1; i <= this.pagedNumber; i++) {
        pages.push(i);
    }
    
    return pages;
  }


  //Fetch records and totalNumber of records present in DB but it is fetching based on current page and
  //items required per page + status of active , inactive or by default null
  getRecordsPerPage(activePage: number, totalRecords: number, status: string | null , sortColumnBy : string | null, order : boolean) {
    this.service.getRecordPerPage(activePage, totalRecords, status, sortColumnBy , order ).subscribe({
      next: (response) => {
        if (response.success) {
            this.userData = response.record; 
            this.activeUserCount = response.totalActiveCount;
            this.inActiveUserCount = response.totalInactiveCount;

            //Done to calculate total number of pages to be showed on pagination UI on basis of totalUser
            if(this.filterStatus == 'active'){
              this.totalUser = response.totalActiveCount; 
            }
            else if(this.filterStatus == 'inactive'){
              this.totalUser = response.totalInactiveCount; 
            }
            else{
              this.totalUser = response.totalUsersCount;
            }

            // Calculate total pages
            this.pagedNumber = Math.ceil(this.totalUser / this.itemsPerPage);
        
          } 
          else {
            this.toastr.error('Error fetching records');
          }
        },
        error: (err) => {
            this.toastr.error(err.error?.message || 'Something went wrong', 'Error!');
        },
    });
  }  



  onSortColumn(column: string): void {
    if (this.sortBy === column) {
      this.isAscending = !this.isAscending;
    } else {
      this.sortBy = column;
      this.isAscending = true;
    }
    this.getRecordsPerPage(this.currentPage, this.itemsPerPage, this.filterStatus, this.sortBy, this.isAscending);
  }



  downloadExcel() {
    this.service.downloadExcel().subscribe({
      next: (response) => {
        const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        // saveAs(blob, 'Users.xlsx');

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'UserData.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);

      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Something went wrong', 'Error!');
      }
    });
  }

  updateClick(id : number) : void {
    this.router.navigate(['/user-management/update-user'], { queryParams: { id: id } });
  }




  updateUserIdToDelete(userId: number){
    this.userIdToDelete = userId;
  }


  deleteUser() : void {
    console.log(this.userIdToDelete);
    this.service.deleteUser(this.userIdToDelete).subscribe({
      next : (res) => {
        if(res.success){
          this.toastr.success("User Deleted", 'Successfully!');

          window.location.reload();
        }
        else {
          this.toastr.error(res.error, 'Error!');
        }
      },

      error : (err) => {
        if(err.error && err.error.message){
          this.toastr.error(err.error.message, 'Error!');
        }
        // else {
        //   this.toastr.error('Something went wrong', 'Error!');
        // }
      }
    })
  }

}

