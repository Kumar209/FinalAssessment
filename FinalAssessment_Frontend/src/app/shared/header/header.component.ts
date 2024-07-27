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


  ngOnInit(): void {
    this.activeHeader = this.activeHeaderList[this.active]
  }

}
