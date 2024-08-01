import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/auth/service/auth.service';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private router : Router, private authService : AuthService){}

  activeHeaderList : any[] = ['dashboard', 'addUser'];

  @Input() active : number = 0;

  activeHeader : string = this.activeHeaderList[this.active];

  userName : string = '';
  imageUrl : string = '';


  ngOnInit(): void {
    this.activeHeader = this.activeHeaderList[this.active]
    var userdetails = JSON.parse(localStorage.getItem('UserDetails') || '');
    this.userName = userdetails.userName;
    this.imageUrl = userdetails.imageUrl;
  }

  getImageUrl(relativePath: string): string {
    var url = `https://localhost:44320/${relativePath}`;
    console.log(url);
    return url;
  }

  
  logout() : void {
    this.authService.logout();

    this.router.navigate(['/auth/login']);
  }

}
