import { Routes } from '@angular/router';
import { UserDirectoryComponent } from '../components/user-directory/user-directory.component';
import { MainframeComponent } from '../components/mainframe/mainframe.component';
import { EmptyComponent } from '../components/empty/empty.component';
import { SignInComponent } from '../components/signin/signin.component';
import { RegisterComponent } from '../components/register/register.component';
import { signedInGuard } from '../guards/signedIn.guard';
import { SandboxComponent } from '../components/sandbox/sandbox.component';
import { ServerErrorComponent } from '../components/server-error/server-error.component';
import { NotFoundComponent } from '../components/not-found/not-found.component';
import { LoremIpsumComponent } from '../components/lorem-ipsum/lorem-ipsum.component';
import { UserProfileComponent } from '../components/users/user-profile/user-profile.component';
import { AdminPanelComponent } from '../components/admin/admin-panel/admin-panel.component';
import { adminGuard } from '../guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [
      { path: 'not-found', component: NotFoundComponent },

      { path: 'sandbox', component: SandboxComponent },
      { path: 'users', component: UserDirectoryComponent },
      { path: 'lorem-ipsum', component: LoremIpsumComponent },
      { path: 'users/:username', component: UserProfileComponent },
      {
        path: 'restricted',
        component: EmptyComponent,
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
      { path: 'user-manager', component: AdminPanelComponent },
      { path: 'server-error', component: ServerErrorComponent },
      { path: '**', component: EmptyComponent, pathMatch: 'full' },
    ],
  },
  { path: 'signin', component: SignInComponent },
  { path: 'register', component: RegisterComponent },

  { path: '**', component: EmptyComponent, pathMatch: 'full' },
];
