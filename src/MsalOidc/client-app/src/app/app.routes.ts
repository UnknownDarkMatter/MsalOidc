import { Routes } from '@angular/router';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { LoginFailedComponent } from './pages/login-failed/login-failed.component';

export const routes: Routes = [
    { path: 'main-page', component: MainPageComponent },
    { path: 'login-failed', component: LoginFailedComponent },
    { path: '**', component: MainPageComponent },  // Wildcard route for a 404 page
];
