import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent implements OnInit{
  public newFileName:string='';
  public files:string[] = [];

  ngOnInit(): void {

  }

  public newFile(){
    this.files.push(this.newFileName);
  }

  public remove(fileIndex:number){
    this.files.splice(fileIndex, 1);
  }
}
