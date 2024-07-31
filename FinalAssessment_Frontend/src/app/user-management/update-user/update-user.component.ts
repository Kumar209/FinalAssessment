import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserManagementService } from '../service/user-management.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-update-user',
  templateUrl: './update-user.component.html',
  styleUrls: ['./update-user.component.css']
})
export class UpdateUserComponent implements OnInit {

  imgSrc : string = '';
  selectedImg : File | null = null; 

  updateUserForm: any;

  userId! : number;

  userDetails : any;

  constructor(private service : UserManagementService, private activatedRoute: ActivatedRoute,  private router: Router, private toastr: ToastrService) {

  }

  
  ngOnInit(){
    this.updateUserForm = new FormGroup({
      firstName: new FormControl('', Validators.required),
      middleName: new FormControl('', Validators.required),
      lastName: new FormControl(''),
      gender: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      dateOfJoining: new FormControl('', Validators.required),
      phone: new FormControl('', Validators.required),
      alternatePhone: new FormControl(''),
      PrashantDbAddresses: new FormArray(
        [
        this.createAddressGroup(1) 
      ]),
      isActive: new FormControl(false)
    });


    //Getting the user id from query params
    this.userId = this.activatedRoute.snapshot.queryParams['id'];


    this.getUserById();



  }

  createAddressGroup(addressTypeId: number): FormGroup {
    return new FormGroup({
      addressTypeId: new FormControl(addressTypeId),
      country: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      zipCode: new FormControl('', Validators.required)
    });
  }


  //This function used to loop through in html for dynamic array of address UI
  get PrashantDbAddresses(): FormArray {
    return this.updateUserForm.get('PrashantDbAddresses') as FormArray;
  }


  addSecondaryAddress() {
    if (this.PrashantDbAddresses.length < 2) {
      this.PrashantDbAddresses.push(this.createAddressGroup(2));
    }
  }

  removeSecondaryAddress() {
    if (this.PrashantDbAddresses.length === 2) {
      this.PrashantDbAddresses.removeAt(1);
    }
  }


  onFileChange(event: any) {
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];
      this.selectedImg = file;
      
      this.imgSrc = URL.createObjectURL(file);
    }
  }

  


  getUserById() {
    this.service.getUserById(this.userId).subscribe({
      next: (response : any) => {
        if (response.success) {
          this.userDetails = response.record;

          console.log(this.userDetails);

          this.patchValueUserDetails();
        } else {
          this.toastr.error(response.message, 'Error!');
        }
      },
      error: (err) => {
        this.toastr.error(err.error.message);
      }
    });
  }


  patchValueUserDetails() {
    this.updateUserForm.patchValue({
      firstName : this.userDetails.firstName,
      middleName : this.userDetails.middleName,
      lastName : this.userDetails.lastName,
      gender : this.userDetails.gender,
      dateOfBirth : this.userDetails.dateOfBirth,
      dateOfJoining : this.userDetails.dateOfJoining,
      email : this.userDetails.email,
      phone : this.userDetails.phone,
      alternatePhone : this.userDetails.alternatePhone
    });


    this.PrashantDbAddresses.clear();

    // Patch addresses
    this.userDetails.addresses.forEach((address: any, index: number) => {
        const addressGroup = this.createAddressGroup(address.addressTypeId);

        addressGroup.patchValue({
          country: address.country,
          state: address.state,
          city: address.city,
          zipCode: address.zipCode
        });

        this.PrashantDbAddresses.push(addressGroup);
    });

  }

  onUpdateUserFormSubmit(){

  }

}
