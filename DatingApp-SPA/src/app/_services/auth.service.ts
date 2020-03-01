import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

baseurl = 'http://localhost:5000/api/auth/login';

constructor(private http: HttpClient) { }

login(model: any) {
  return this.http.post(this.baseurl , model).pipe
  (
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
      }
    })
  );
 }
}
