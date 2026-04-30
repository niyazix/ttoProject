import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { permissionGuard } from '../../core/guards/permission.guard';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';

const routes: Routes = [
  { path: 'login', loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule) },
  { path: 'unauthorized', loadChildren: () => import('./pages/unauthorized/unauthorized.module').then(m => m.UnauthorizedModule) },
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardModule) },
      { path: 'pages', canActivate: [permissionGuard], data: { permission: 'pages.view' }, loadChildren: () => import('./pages/pages-manager/pages-manager.module').then(m => m.PagesManagerModule) },
      { path: 'news', canActivate: [permissionGuard], data: { permission: 'news.view' }, loadChildren: () => import('./pages/news-manager/news-manager.module').then(m => m.NewsManagerModule) },
      { path: 'announcements', canActivate: [permissionGuard], data: { permission: 'announcements.view' }, loadChildren: () => import('./pages/announcements-manager/announcements-manager.module').then(m => m.AnnouncementsManagerModule) },
      { path: 'contact', canActivate: [permissionGuard], data: { permission: 'contact.view' }, loadChildren: () => import('./pages/contact-manager/contact-manager.module').then(m => m.ContactManagerModule) },
      { path: 'menu', canActivate: [permissionGuard], data: { permission: 'menu.manage' }, loadChildren: () => import('./pages/menu-manager/menu-manager.module').then(m => m.MenuManagerModule) },
      { path: 'users', canActivate: [permissionGuard], data: { permission: 'users.manage' }, loadChildren: () => import('./pages/users-manager/users-manager.module').then(m => m.UsersManagerModule) },
      { path: 'roles', canActivate: [permissionGuard], data: { permission: 'roles.manage' }, loadChildren: () => import('./pages/roles-manager/roles-manager.module').then(m => m.RolesManagerModule) },
      { path: 'settings', canActivate: [permissionGuard], data: { permission: 'settings.edit' }, loadChildren: () => import('./pages/settings/settings.module').then(m => m.SettingsModule) },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule {}
