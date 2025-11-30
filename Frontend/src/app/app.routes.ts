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
import { UserManagerComponent } from './components/admin/user-manager/user-manager.component';
import { RoasterManagerComponent } from './components/admin/roaster-manager/roaster-manager.component';
import { RoasterRevisionComponent } from './components/roaster-revision/roaster-revision.component';
import { RevisionHistoryComponent } from './components/revision-history/revision-history.component';
import { RoasterFrameComponent } from './components/roaster-frame/roaster-frame.component';
import { UserFrameComponent } from './components/user-frame/user-frame.component';
import { UserContributionsComponent } from './components/user-contributions/user-contributions.component';
import { UserRevisionsComponent } from './components/user-revisions/user-revisions.component';
import { isUserGuard } from './guards/is-user.guard';
import { RoasterRevisionEditComponent } from './components/roaster-revision-edit/roaster-revision-edit.component';
import { RevisionsManagerComponent } from './components/moderator/revisions-manager/revisions-manager.component';
import { RoasterRevisionReviewComponent } from './components/moderator/roaster-revision-review/roaster-revision-review.component';
import { moderatorGuard } from './guards/moderator.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [
      { path: 'roasters', component: RoasterDirectoryComponent },
      {
        path: 'roasters/create',
        component: RoasterCreateComponent,
        canActivate: [signedInGuard],
      },
      {
        path: 'roasters/:id',
        component: RoasterFrameComponent,
        children: [
          {
            path: 'revisions/:revisionId/edit',
            component: RoasterRevisionEditComponent,
          },
          {
            path: 'revisions/:revisionId1/:revisionId2',
            component: RoasterRevisionComponent,
          },
          {
            path: 'revisions/:revisionId1',
            component: RoasterRevisionComponent,
          },
          {
            path: 'revisions',
            component: RevisionHistoryComponent,
          },
          {
            path: 'edit',
            component: RoasterEditComponent,
            canActivate: [signedInGuard],
          },
          { path: '', component: RoasterDetailsComponent },
        ],
      },
      { path: 'not-found', component: NotFoundComponent },
      { path: 'sandbox', component: SandboxComponent },
      { path: 'lorem-ipsum', component: LoremIpsumComponent },
      { path: 'users', component: UserDirectoryComponent },
      {
        path: 'users/:username',
        component: UserFrameComponent,
        children: [
          {
            path: 'revisions',
            component: UserRevisionsComponent,
            canActivate: [isUserGuard],
          },
          { path: 'contributions', component: UserContributionsComponent },
          { path: '', component: UserProfileComponent },
        ],
      },
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
  {
    path: 'mod',
    component: MainframeComponent,
    canActivateChild: [moderatorGuard],
    children: [
      { path: 'revisions-manager', component: RevisionsManagerComponent },
      {
        path: 'roaster-revision-review',
        component: RoasterRevisionReviewComponent,
      },
      { path: 'user-revisions', component: UserRevisionsComponent },
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
