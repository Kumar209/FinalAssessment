import { Component , ChangeDetectorRef} from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserManagementService } from '../service/user-management.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Country, State, City, ICountry, IState, ICity} from 'country-state-city';


@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.css']
})
export class AddUserComponent {



  imgSrc : string = '';
  selectedImg : File | null = null; 

  addUserForm: any;

  //For the dropdown cascading
  countries : ICountry[] = []; 

  states: IState[][] = []; 
  cities: ICity[][] = [];





  selectedCountryCode: string = '';


  constructor(private service : UserManagementService,  private cd: ChangeDetectorRef, private router: Router, private toastr: ToastrService) {

  }
  
  ngOnInit() {
    this.countries = Country.getAllCountries();

    this.addUserForm = new FormGroup({
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
        [
        this.createAddressGroup(1) 
      ]),
    });

    this.loadStaticData();

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
    return this.addUserForm.get('PrashantDbAddresses') as FormArray;
  }


  addSecondaryAddress() {
    if (this.PrashantDbAddresses.length < 2) {
      this.PrashantDbAddresses.push(this.createAddressGroup(2));

      this.states.push([]); 
      this.cities.push([]); 
    }
  }

  removeSecondaryAddress() {
    if (this.PrashantDbAddresses.length === 2) {
      this.PrashantDbAddresses.removeAt(1);

      this.states.pop(); 
      this.cities.pop(); 

    }
  }


  loadStaticData() {
    this.countries = Country.getAllCountries(); 
  }



  onCountryChange(index: number) {
    const addressControl = this.PrashantDbAddresses.at(index) as FormGroup;
    const countryName = addressControl.get('country')?.value;
    const countryDetail = this.countries.find(c => c.name === countryName);

    if (countryName) {
      this.states[index] = State.getStatesOfCountry(countryDetail?.isoCode); // Update states for specific address
      this.cities[index] = []; // Clear cities when country changes

      addressControl.get('state')?.setValue('');
      addressControl.get('state')?.enable();

      addressControl.get('city')?.setValue('');
      addressControl.get('city')?.disable();
    }
  }

  onStateChange(index: number) {
    const addressControl = this.PrashantDbAddresses.at(index) as FormGroup;
    const stateName = addressControl.get('state')?.value;
    const countryName = addressControl.get('country')?.value;
    const countryDetail = this.countries.find(c => c.name === countryName);
    const stateDetail = this.states[index].find(s => s.name === stateName);

    if (stateName && countryDetail && stateDetail) {
      this.cities[index] = City.getCitiesOfState(countryDetail.isoCode, stateDetail.isoCode); // Update cities for specific address

      addressControl.get('city')?.setValue('');
      addressControl.get('city')?.enable();
    } else {
      addressControl.get('city')?.setValue('');
      addressControl.get('city')?.disable();
    }
  }

  getStates(index: number): IState[] {
    return this.states[index] || [];
  }

  getCities(index: number): ICity[] {
    return this.cities[index] || [];
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

  validateImage(): boolean {
    return this.selectedImg !== null;
  }





  onAddUserFormSubmit(){
    if (!this.validateImage()) {
      this.toastr.error('Image file is required', 'Error!');
      return;
    }



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

    this.PrashantDbAddresses.controls.forEach((control, index) => {
      const addressGroup = control as FormGroup;
      const addressPrefix = `PrashantDbAddresses[${index}]`;
      
      formData.append(`${addressPrefix}.addressTypeId`, addressGroup.get('addressTypeId')?.value);
      formData.append(`${addressPrefix}.country`, addressGroup.get('country')?.value);
      formData.append(`${addressPrefix}.state`, addressGroup.get('state')?.value);
      formData.append(`${addressPrefix}.city`, addressGroup.get('city')?.value);
      formData.append(`${addressPrefix}.zipCode`, addressGroup.get('zipCode')?.value);
    });



    if (this.selectedImg) {
      formData.append('ImageFile', this.selectedImg, this.selectedImg.name);
    }

  


    this.service.addUser(formData).subscribe({
      next : (response) => {
        if(response.success){
          this.toastr.success(response.message, 'Successfully!');
          this.selectedImg = null;
          this.imgSrc = '';
          this.addUserForm.reset();
          this.PrashantDbAddresses.clear();
          this.PrashantDbAddresses.push(this.createAddressGroup(1));
          this.states = [];
          this.cities = [];
          this.router.navigate(['/user-management/dashboard']);
        }
        else{
          this.toastr.error(response.message, 'Error!');
        }
      },

      error : (err) => {
        if(err.error && err.error.message){
          this.toastr.error(err.error.message, 'Error!');
        }
        else{
          this.toastr.error('Something went wrong', 'Error!');
        }
      }
    })


  
    
  }
}
