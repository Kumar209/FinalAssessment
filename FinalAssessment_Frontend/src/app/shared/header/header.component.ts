import { Component, Input, OnInit } from '@angular/core';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

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
    var url = `http://localhost:44320/${relativePath}`;
    console.log(url);
    return url
  }

}
