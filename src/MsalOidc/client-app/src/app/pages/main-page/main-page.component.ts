import { Component, Inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpService } from '../../services/http-service';
import {FileDto} from '../../model/file-dto';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [FormsModule, CommonModule, HttpClientModule],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss',
  providers:[HttpService]
})
export class MainPageComponent implements OnInit{
  public newFileName:string='';
  public files:FileDto[] = [];

constructor(private httpService:HttpService){}

  ngOnInit(): void {
    this.httpService.getAll<FileDto[]>('File/GetAll')
    .subscribe((files) => this.files = files);
  }

  public newFile(){
    let newFile:FileDto = {
      id:-1,
      friendlyName:this.newFileName,
      hashedName:''
    };
    this.httpService.create<FileDto>(newFile, 'File/Create')
    .subscribe(() => {
      this.httpService.getAll<FileDto[]>('File/GetAll')
      .subscribe((files) => this.files = files);
    });
  }

  public remove(fileIndex:number){
    this.httpService.delete(fileIndex, 'File/Delete')
    .subscribe(() => {
      this.httpService.getAll<FileDto[]>('File/GetAll')
      .subscribe((files) => this.files = files);
    });
  }
}
