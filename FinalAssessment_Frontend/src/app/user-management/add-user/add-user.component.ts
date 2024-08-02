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
  states: IState[] = [];
  cities: ICity[] = [];





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
      dateOfBirth: new FormControl('', Validators.required),
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

    // Initialize event listeners for address controls
    this.PrashantDbAddresses.controls.forEach((control, index) => {
      this.onCountryChange(index);
      this.onStateChange(index);
    });

  }

  createAddressGroup(addressTypeId: number): FormGroup {
    return new FormGroup({
      addressTypeId: new FormControl(addressTypeId),
      country: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      zipCode: new FormControl('', [Validators.required, Validators.maxLength(6), Validators.minLength(6), Validators.pattern('^[0-9]*$')])
    });
  }


  //This function used to loop through in html for dynamic array of address UI
  get PrashantDbAddresses(): FormArray {
    return this.addUserForm.get('PrashantDbAddresses') as FormArray;
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


  loadStaticData() {
    this.countries = Country.getAllCountries(); 
    this.states = []; 
    this.cities = []; 
  }



  

  onCountryChange(index: number) {
    const addressControl = this.PrashantDbAddresses.at(index) as FormGroup;  

    addressControl.get('country')?.valueChanges.subscribe((countryName: string) => {

      const countryDetail = this.countries.find(c => c.name === countryName);

        if (countryName) {
        // Fetch states based on the selected country
        this.states = State.getStatesOfCountry(countryDetail?.isoCode);
  
        // Update the state control for the specific address group
        const stateControl = addressControl.get('state');
        stateControl?.setValue('');
        stateControl?.enable();
  
        // Update the city control for the specific address group
        const cityControl = addressControl.get('city');
        cityControl?.setValue('');
        cityControl?.disable();
      }
    });
  }
    



  onStateChange(index: number) {
    const addressControl = this.PrashantDbAddresses.at(index) as FormGroup;
  
    addressControl.get('state')?.valueChanges.subscribe((stateName: string) => {
      const countryName = addressControl.get('country')?.value;

      //Here getting all detail using country name
      const countryDetail = this.countries.find(c => c.name === countryName);

  
      // Finding the state detail based on the selected state name
      const stateDetail = this.states.find(s => s.name === stateName);
  
      if (stateName && countryDetail && stateDetail) {
        
        this.cities = City.getCitiesOfState(countryDetail.isoCode, stateDetail.isoCode);
  
        // Reset the city control
        addressControl.get('city')?.setValue('');
        addressControl.get('city')?.enable();
      } 
      else {
        // If no valid state or country is selected, disable the city control
        addressControl.get('city')?.setValue('');
        addressControl.get('city')?.disable();
      }
    });
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

    console.log(this.addUserForm.value);

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


    // resetForm(){
    //   this.selectedImg = null;
    //   this.imgSrc = '';
    //   this.addUserForm.reset();
    //   this.PrashantDbAddresses.clear();
    //   this.PrashantDbAddresses.push(this.createAddressGroup(1));
    //   this.addressStates = [];
    //   this.addressCities = [];
    //   this.router.navigate(['/user-management/dashboard']);
    // }
  


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
