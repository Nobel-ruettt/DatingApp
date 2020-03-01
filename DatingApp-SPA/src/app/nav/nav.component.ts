import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: {
    name: string,
    password: string
  };
  constructor(private authservice: AuthService) { }
  ngOnInit() {
  }
  login() {
    this.authservice.login(this.model).subscribe(
      next => {
        console.log('Login successful');
      },
      error => {
        console.log('Failed to login');
      }
    );
  }
}
