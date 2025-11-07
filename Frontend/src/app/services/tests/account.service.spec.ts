import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { AccountService } from '../account.service';
import { provideHttpClient } from '@angular/common/http';
import { User } from '../../models/user';
import {
  ChangePasswordPayload,
  ChangeUsernamePayload,
  RegisterPayload,
  SignInPayload,
} from '../../models/account';
import { environment } from '../../../environments/environment';
import { expectHttpSignalUpdate } from '../../utils/test.utils';

describe('AccountService', () => {
  let accountService: AccountService;
  let user: User;
  let adminUser: User;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AccountService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    accountService = TestBed.inject(AccountService);
    httpTesting = TestBed.inject(HttpTestingController);

    const userFakeJwtPayload = {
      nameid: '56',
      unique_name: 'johndoe',
      role: [],
    };
    const adminUserFakeJwtPayload = {
      nameid: '57',
      unique_name: 'admin',
      role: ['Moderator', 'Admin'],
    };

    user = {
      id: Number(userFakeJwtPayload.nameid),
      username: userFakeJwtPayload.unique_name,
      token: `header.${btoa(JSON.stringify(userFakeJwtPayload))}.signature`,
    };
    adminUser = {
      id: Number(adminUserFakeJwtPayload.nameid),
      username: adminUserFakeJwtPayload.unique_name,
      token: `header.${btoa(JSON.stringify(adminUserFakeJwtPayload))}.signature`,
    };
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('create a service', () => {
    expect(accountService).toBeTruthy();
  });

  describe('setCurrentUser', () => {
    it('should set current user', () => {
      accountService.setCurrentUser(user);
      expect(accountService.currentUser()).toEqual(user);
      expect(localStorage.getItem('user')).toEqual(JSON.stringify(user));
    });
  });

  describe('signOut', () => {
    it('should sign out current user', () => {
      accountService.setCurrentUser(user);
      accountService.signOut();
      expect(accountService.currentUser()).toBeNull();
      expect(localStorage.getItem('user')).toBeNull();
    });
  });

  describe('roles', () => {
    it('should return no roles from user JWT', () => {
      accountService.setCurrentUser(user);
      expect(accountService.roles()).toEqual([]);
    });
    it('should return staff roles from user JWT', () => {
      accountService.setCurrentUser(adminUser);
      expect(accountService.roles()).toEqual(
        expect.arrayContaining(['Admin', 'Moderator']),
      );
    });
  });

  describe('signIn', () => {
    it('should sign in and set currentUser', () => {
      const signInPayload: SignInPayload = { username: '', password: '' };
      expectHttpSignalUpdate(
        httpTesting,
        accountService.signIn(signInPayload),
        'account/login',
        user,
      );
      expect(accountService.currentUser()).toEqual(user);
    });
  });

  describe('register', () => {
    it('should register and set current user', () => {
      const registerPayload: RegisterPayload = {
        username: '',
        password: '',
        confirmPassword: '',
      };
      expectHttpSignalUpdate(
        httpTesting,
        accountService.register(registerPayload),
        'account/register',
        user,
      );
      expect(accountService.currentUser()).toEqual(user);
    });
  });

  describe('changeUsername', () => {
    it('should change username of user and update current user', () => {
      const changeUsernamePayload: ChangeUsernamePayload = {
        newUsername: '',
        password: '',
      };
      expectHttpSignalUpdate(
        httpTesting,
        accountService.changeUsername(changeUsernamePayload),
        'account/change-username',
        user,
      );
      expect(accountService.currentUser()).toEqual(user);
    });
  });

  describe('changePassword', () => {
    it('should change password of user and update current user', () => {
      const changePasswordPayload: ChangePasswordPayload = {
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: '',
      };
      expectHttpSignalUpdate(
        httpTesting,
        accountService.changePassword(changePasswordPayload),
        'account/change-password',
        user,
      );
      expect(accountService.currentUser()).toEqual(user);
    });
  });
});
