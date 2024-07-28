import { Component } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.css']
})
export class AddUserComponent {
  imgSrc : string = '';
  selectedImg : File | null = null; 

  addUserForm: any;

  ngOnInit() {
    this.addUserForm = new FormGroup({
      firstName: new FormControl('', Validators.required),
      middleName: new FormControl('', Validators.required),
      lastName: new FormControl(''),
      gender: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      dateOfJoining: new FormControl('', Validators.required),
      phone: new FormControl('', Validators.required),
      alternatePhone: new FormControl(''),
      addresses: new FormArray([
        this.createAddressGroup(1) 
      ]),
      isActive: new FormControl(true)
    });
  }

  createAddressGroup(addressTypeId: number): FormGroup {
    return new FormGroup({
      addressTypeId: new FormControl(addressTypeId),
      country: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      // zipCode: new FormControl('', Validators.required)
      zipCode : new FormControl('262701')
    });
  }


  //This function used to loop through in html for dynamic array of address UI
  get addresses(): FormArray {
    return this.addUserForm.get('addresses') as FormArray;
  }


  addSecondaryAddress() {
    if (this.addresses.length < 2) {
      this.addresses.push(this.createAddressGroup(2));
    }
  }

  removeSecondaryAddress() {
    if (this.addresses.length === 2) {
      this.addresses.removeAt(1);
    }
  }




  onFileChange(event: any) {
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];
      this.selectedImg = file;
      this.imgSrc = URL.createObjectURL(file);
    }
  }

  onAddUserFormSubmit(){
    console.log(this.addUserForm.value);

    const formData = new FormData();
    formData.append('firstName', this.addUserForm.get('firstName').value);
    formData.append('middleName', this.addUserForm.get('middleName').value);
    formData.append('lastName', this.addUserForm.get('lastName').value);
    formData.append('gender', this.addUserForm.get('gender').value);
    formData.append('dateOfBirth', this.addUserForm.get('dateOfBirth').value);
    formData.append('email', this.addUserForm.get('email').value);
    formData.append('dateOfJoining', this.addUserForm.get('dateOfJoining').value);
    formData.append('phone', this.addUserForm.get('phone').value);
    formData.append('alternatePhone', this.addUserForm.get('alternatePhone').value);
    formData.append('isActive', this.addUserForm.get('isActive').value);

    this.addresses.controls.forEach((control, index) => {
      const addressGroup = control as FormGroup;
      const addressPrefix = `addresses[${index}]`;
      formData.append(`${addressPrefix}.addressTypeId`, addressGroup.get('addressTypeId')?.value);
      formData.append(`${addressPrefix}.country`, addressGroup.get('country')?.value);
      formData.append(`${addressPrefix}.state`, addressGroup.get('state')?.value);
      formData.append(`${addressPrefix}.city`, addressGroup.get('city')?.value);
      formData.append(`${addressPrefix}.street`, addressGroup.get('street')?.value);
      formData.append(`${addressPrefix}.zipCode`, addressGroup.get('zipCode')?.value);
    });


    if (this.selectedImg) {
      formData.append('ImageFile', this.selectedImg, this.selectedImg.name);
    }
  }
}
