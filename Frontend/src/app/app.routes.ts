import { Routes } from '@angular/router';
import { UserDirectoryComponent } from './components/user-directory/user-directory.component';
import { MainframeComponent } from './components/mainframe/mainframe.component';
import { EmptyComponent } from './components/empty/empty.component';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [{ path: 'users', component: UserDirectoryComponent }],
  },
  { path: '**', component: EmptyComponent, pathMatch: 'full' },
];
