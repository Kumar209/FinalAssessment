import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserManagementService } from '../service/user-management.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Country, State, City, ICountry, IState, ICity } from 'country-state-city';

@Component({
  selector: 'app-update-user',
  templateUrl: './update-user.component.html',
  styleUrls: ['./update-user.component.css']
})
export class UpdateUserComponent implements OnInit {

  imgSrc : string = '';
  selectedImg : File | null = null; 

  prevImageFile! : File;

  updateUserForm: any;

  userId! : number;

  userDetails : any;


  countries: ICountry[] = [];
  states: IState[][] = [];
  cities: ICity[][] = [];


  constructor(private service : UserManagementService, private activatedRoute: ActivatedRoute,  private router: Router, private toastr: ToastrService) {

  }
  
  convertImageToFile(dataUrl : string) :Promise<File> {
    return fetch(dataUrl)
    .then(res => res.blob())
    .then(blob => {
      const fileName = `image_${new Date().getTime()}.png`;
      return new File([blob], fileName, { type: blob.type });
    });
  }



  validateImage(): boolean {
    var updatedImage = this.selectedImg !== null;
    var oldImage = this.imgSrc !== null;
    return (oldImage || updatedImage);
  }
  
  ngOnInit(){
    this.updateUserForm = new FormGroup({
      firstName: new FormControl('', [Validators.required,  Validators.pattern('^[a-zA-Z]*$')]),
      middleName: new FormControl('', [Validators.required,  Validators.pattern('^[a-zA-Z]*$')]),
      lastName: new FormControl('', [Validators.pattern('^[a-zA-Z]*$')]),
      gender: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', [Validators.required, this.dateValidator]),
      email: new FormControl('', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]),
      dateOfJoining: new FormControl('', Validators.required),
      phone: new FormControl('', Validators.required),
      alternatePhone: new FormControl(''),
      PrashantDbAddresses: new FormArray(
        [ ]),
    });


    this.countries = Country.getAllCountries();

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
      zipCode: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{6}$')])
    });
  }


  //This function used to loop through in html for dynamic array of address UI
  get PrashantDbAddresses(): FormArray {
    return this.updateUserForm.get('PrashantDbAddresses') as FormArray;
  }


  addSecondaryAddress() {
    if (this.PrashantDbAddresses.length < 2) {
      this.PrashantDbAddresses.push(this.createAddressGroup(2));
      this.states.push([]);
      this.cities.push([]);
      this.subscribeToAddressChanges(this.PrashantDbAddresses.length - 1);
    }
  }

  removeSecondaryAddress() {

    if (this.PrashantDbAddresses.length > 1) { 
      this.PrashantDbAddresses.removeAt(this.PrashantDbAddresses.length - 1); 
      this.states.pop();
      this.cities.pop();
    }
  }


   //Method to restrict the user selecting the date after the current date
   getMaxDate(): string {
    const currentDate = new Date();

    // Format the current date as YYYY-MM-DD (required format for the max attribute)
    const formattedDate = currentDate.toISOString().split('T')[0];

    return formattedDate;
  }

   // Custom date validator to check for future dates
   dateValidator(control: AbstractControl): { [key: string]: any } | null {
    const inputDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Set to midnight

    if (inputDate > today) {
      return { 'max': true };
    }
    return null;
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

    this.imgSrc = `https://localhost:44320/${this.userDetails.imageUrl}`;

    

    this.PrashantDbAddresses.clear();
    this.states = [];
    this.cities = [];
      

    if (this.userDetails.prashantDbAddresses && this.userDetails.prashantDbAddresses.length > 0) {

      this.userDetails.prashantDbAddresses.forEach((address: any, index: number) => {

        const addressGroup = this.createAddressGroup(address.addressTypeId);
        
        addressGroup.patchValue({
          country: address.country,
          state: address.state,
          city: address.city,
          zipCode: address.zipCode
        });

        this.PrashantDbAddresses.push(addressGroup);
        this.states.push([]);
        this.cities.push([]);


        this.subscribeToAddressChanges(index);

        const countryDetail = this.countries.find(c => c.name === address.country);
        if (countryDetail) {
          this.states[index] = State.getStatesOfCountry(countryDetail.isoCode);
        }

        const stateDetail = this.states[index].find(s => s.name === address.state);
        if (stateDetail && countryDetail) {
          this.cities[index] = City.getCitiesOfState(countryDetail.isoCode, stateDetail.isoCode);
        }

      });
    }
  }


  subscribeToAddressChanges(index: number) {
    const addressGroup = this.PrashantDbAddresses.at(index) as FormGroup;
    const countryControl = addressGroup.get('country');
    const stateControl = addressGroup.get('state');
    const cityControl = addressGroup.get('city');

    if (countryControl && stateControl && cityControl) {
      countryControl.valueChanges.subscribe(countryName => {
        const countryDetail = this.countries.find(c => c.name === countryName);
        if (countryDetail) {
          this.states[index] = State.getStatesOfCountry(countryDetail.isoCode);
          stateControl.setValue(''); 
          cityControl.setValue('');
        } else {
          this.states[index] = [];
          this.cities[index] = [];
        }
      });

      stateControl.valueChanges.subscribe(stateName => {
        const countryName = countryControl.value;
        const countryDetail = this.countries.find(c => c.name === countryName);
        if (countryDetail) {
          const stateDetail = this.states[index].find(s => s.name === stateName);
          if (stateDetail) {
            this.cities[index] = City.getCitiesOfState(countryDetail.isoCode, stateDetail.isoCode);
            cityControl.setValue(''); 
          } else {
            this.cities[index] = [];
          }
        } else {
          this.cities[index] = [];
        }
      });
    }
  }




  async onUpdateUserFormSubmit(){
    if (!this.validateImage()) {
      this.toastr.error('Image file is required', 'Error!');
      return;
    }

    
    const formData = new FormData();

    formData.append('Id', this.userId.toString());

    formData.append('firstName', this.updateUserForm.get('firstName').value);
    formData.append('middleName', this.updateUserForm.get('middleName').value);
    formData.append('lastName', this.updateUserForm.get('lastName').value);
    formData.append('gender', this.updateUserForm.get('gender').value);
    formData.append('dateOfBirth', this.updateUserForm.get('dateOfBirth').value);
    formData.append('email', this.updateUserForm.get('email').value);
    formData.append('dateOfJoining', this.updateUserForm.get('dateOfJoining').value);
    formData.append('phone', this.updateUserForm.get('phone').value);
    formData.append('alternatePhone', this.updateUserForm.get('alternatePhone').value ?? '');

    // Append addresses
    this.PrashantDbAddresses.controls.forEach((control, index) => {
      const addressGroup = control as FormGroup;
      const addressPrefix = `PrashantDbAddresses[${index}]`;

      formData.append(`${addressPrefix}.addressTypeId`, addressGroup.get('addressTypeId')?.value);
      formData.append(`${addressPrefix}.country`, addressGroup.get('country')?.value);
      formData.append(`${addressPrefix}.state`, addressGroup.get('state')?.value);
      formData.append(`${addressPrefix}.city`, addressGroup.get('city')?.value);
      formData.append(`${addressPrefix}.zipCode`, addressGroup.get('zipCode')?.value);
    });

    
    //If user upload img again
    if (this.selectedImg) {
      formData.append('ImageFile', this.selectedImg, this.selectedImg.name);
    }

    //If user doesn't want to change the uploaded img
    else{
      const file = await this.convertImageToFile(this.imgSrc);
      formData.append('ImageFile', file, file.name);
    }




    //Here we have to call api of update
    this.service.updateUser(formData).subscribe({
      next : (res : any) => {
        if(res.success){
          this.router.navigate(['/user-management/dashboard']);
          this.toastr.success(res.message, 'Succesfully!');
        }
        else {
          this.toastr.error(res.message, 'Error!')
        }
      },

      error : (err) => {
        if(err.error && err.error.message){
          this.toastr.error(err.error.message)
        }
        else{
          this.toastr.error('Something went wrong', 'Error!');
        }
      }
    })
  }
}
