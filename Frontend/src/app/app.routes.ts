import { Routes } from '@angular/router';
import { UserDirectoryComponent } from './components/user-directory/user-directory.component';
import { MainframeComponent } from './components/mainframe/mainframe.component';
import { EmptyComponent } from './components/empty/empty.component';
import { SignInComponent } from './components/signin/signin.component';
import { RegisterComponent } from './components/register/register.component';
import { signedInGuard } from './guards/signedIn.guard';
import { SandboxComponent } from './components/sandbox/sandbox.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { LoremIpsumComponent } from './components/lorem-ipsum/lorem-ipsum.component';
import { UserProfileComponent } from './components/users/user-profile/user-profile.component';
import { adminGuard } from './guards/admin.guard';
import { UserSettingsComponent } from './components/user-settings/user-settings.component';
import { RoasterDirectoryComponent } from './components/roaster-directory/roaster-directory.component';
import { RoasterDetailsComponent } from './components/roaster-details/roaster-details.component';
import { RoasterCreateComponent } from './components/roaster-create/roaster-create.component';
import { RoasterEditComponent } from './components/roaster-edit/roaster-edit.component';
import { RoasterManagerComponent } from './components/admin/roaster-manager/roaster-manager.component';
import { UserManagerComponent } from './components/admin/user-manager/user-manager.component';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [
      { path: 'roasters', component: RoasterDirectoryComponent },
      { path: 'roasters/create', component: RoasterCreateComponent },
      { path: 'roasters/:id', component: RoasterDetailsComponent },
      { path: 'roasters/edit/:id', component: RoasterEditComponent },
      { path: 'not-found', component: NotFoundComponent },
      { path: 'sandbox', component: SandboxComponent },
      { path: 'lorem-ipsum', component: LoremIpsumComponent },
      { path: 'users', component: UserDirectoryComponent },
      { path: 'users/:username', component: UserProfileComponent },
      {
        path: 'restricted',
        component: EmptyComponent,
        canActivate: [signedInGuard],
      },
      {
        path: 'settings',
        component: UserSettingsComponent,
        canActivate: [signedInGuard],
      },
    ],
  },
  // admin only routes
  {
    path: 'admin',
    runGuardsAndResolvers: 'always',
    component: MainframeComponent,
    canActivateChild: [adminGuard],
    children: [
      { path: 'roasters', component: RoasterManagerComponent },
      { path: 'users', component: UserManagerComponent },
      { path: 'server-error', component: ServerErrorComponent },
      { path: '**', component: EmptyComponent, pathMatch: 'full' },
    ],
  },
  { path: 'signin', component: SignInComponent },
  { path: 'register', component: RegisterComponent },

  { path: '**', component: EmptyComponent, pathMatch: 'full' },
];
