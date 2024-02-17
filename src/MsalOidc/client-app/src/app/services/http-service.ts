import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import{environment} from '../../environments/environment';
import { EndPoint } from './enpoints';



@Injectable({
  providedIn: 'root',
})
export class HttpService {

  apiURL = environment.apiConfig.uri;
  constructor(private http: HttpClient) {}

  
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };


  getAll<T>(relaviteUrl:EndPoint): Observable<T> {
    return this.http
      .get<T>(this.apiURL + '/' + relaviteUrl);;
  }

  create<T>(data: T, relaviteUrl:EndPoint): Observable<T> {
    return this.http
      .post<T>(
        this.apiURL + '/' + relaviteUrl,
        JSON.stringify(data),
        this.httpOptions
      );
  }
 
  update<T>(data: T, relaviteUrl:EndPoint): Observable<T> {
    return this.http
      .put<T>(
        this.apiURL + '/' + relaviteUrl,
        JSON.stringify(data),
        this.httpOptions
      );
  }

  delete(id: number, relaviteUrl:EndPoint) {
    return this.http
      .delete(`${this.apiURL}/${relaviteUrl}?id=${id}`, this.httpOptions);
  }

  
}