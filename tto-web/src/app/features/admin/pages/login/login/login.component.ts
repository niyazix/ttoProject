import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  form: FormGroup;
  loading   = false;
  error     = '';
  showPass  = false;

  constructor(fb: FormBuilder, private auth: AuthService, private router: Router) {
    if (auth.isLoggedIn()) router.navigate(['/admin']);
    this.form = fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.error   = '';

    this.auth.login(this.form.value).subscribe({
      next:  () => this.router.navigate(['/admin']),
      error: err => {
        this.error   = err?.error?.message ?? 'Giriş başarısız. Bilgilerinizi kontrol edin.';
        this.loading = false;
      }
    });
  }

  f(name: string) { return this.form.get(name); }
  isInvalid(name: string) { const c = this.f(name); return c?.invalid && c?.touched; }
}
