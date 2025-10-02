import { Routes } from '@angular/router';
import { UserDirectoryComponent } from './components/user-directory/user-directory.component';
import { MainframeComponent } from './components/mainframe/mainframe.component';
import { EmptyComponent } from './components/empty/empty.component';
import { SignInComponent } from './components/signin/signin.component';
import { RegisterComponent } from './components/register/register.component';
import { signedInGuard } from './guards/signedIn.guard';
import { TestErrorsComponent } from './components/test-errors/test-errors.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';
import { NotFoundComponent } from './components/not-found/not-found.component';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [
      { path: 'not-found', component: NotFoundComponent },
      { path: 'server-error', component: ServerErrorComponent },
      { path: 'test-errors', component: TestErrorsComponent },
      { path: 'users', component: UserDirectoryComponent },
      {
        path: 'restricted',
        component: EmptyComponent,
        canActivate: [signedInGuard],
      },
    ],
  },
  { path: 'signin', component: SignInComponent },
  { path: 'register', component: RegisterComponent },

  { path: '**', component: EmptyComponent, pathMatch: 'full' },
];
