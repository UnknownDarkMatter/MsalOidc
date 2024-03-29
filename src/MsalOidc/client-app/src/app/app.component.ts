import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { AuthComponent } from './components/auth/auth.component';
import { LoginFailedComponent } from './pages/login-failed/login-failed.component';

 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, 
    AuthComponent,
    HeaderComponent, 
    MainPageComponent,
    LoginFailedComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'MsalDemo';
}
