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
  constructor(
    private serivce: UserManagementService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  userData: any[] = [];

  //Used to track current page number
  currentPage: number = 1;

  //used to set how many organizaton should be shown on a single page of table
  itemsPerPage: number = 3;

  ngOnInit(): void {
    this.getRecordsPerPage(1, this.itemsPerPage);
  }

  getRecordsPerPage(activePage : number, totalRecords : number) {
    this.serivce
      .getRecordPerPage(activePage, totalRecords)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.userData = response.record;
          } else {
            this.toastr.error('Error fetching records');
          }
        },

        error: (err) => {
          if (err.error && err.error.message) {
            this.toastr.error(err.error.message, 'Error!');
          } else {
            this.toastr.error('Something went wrong', 'Error!');
          }
        },
      });
  }


  //These two function are filtering address type
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

  //Method used to give only 10 patient details as an array to populate table
  //Method slice the organizationInfoData our json patient data based on startIndex
  //  paginatedPatientInfo(): any[] {
  //    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
  //    return this.organizationInfoData.slice(startIndex, startIndex + this.itemsPerPage);
  //  }

  //Method used to update the current page when clicked
  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
  }

  //Method used to calculate total number of pages
  totalPages(): number[] {
    return Array(Math.ceil(this.userData.length / this.itemsPerPage))
      .fill(0)
      .map((x, i) => i + 1);
  }
}
