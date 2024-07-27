import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {


  userData : any[] = ['d', 'ds', 'ds']

 //Used to track current page number
 currentPage: number = 1;

 //used to set how many organizaton should be shown on a single page of table
 itemsPerPage: number = 5;


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
   return Array(Math.ceil(this.userData.length / this.itemsPerPage)).fill(0).map((x, i) => i + 1);
 }
}
