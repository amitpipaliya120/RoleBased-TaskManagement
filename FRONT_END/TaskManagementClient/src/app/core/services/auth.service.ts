import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();
  constructor(private http: HttpClient) { }
  login(credentials: { email: string, password: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('jwt_token', response.token);
          localStorage.setItem('user_role', response.role);
          localStorage.setItem('user_id', response.userId);
          localStorage.setItem('user_name', response.name);
          this.isLoggedInSubject.next(true);
        }
      })
    );
  }
  logout(): void {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_role');
    localStorage.removeItem('user_id');
    localStorage.removeItem('user_name');
    this.isLoggedInSubject.next(false);
  }
  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }
  getRole(): string | null {
    return localStorage.getItem('user_role');
  }
  getName(): string | null {
    return localStorage.getItem('user_name');
  }
  private hasToken(): boolean {
    return !!localStorage.getItem('jwt_token');
  }
}
