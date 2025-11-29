import { TestBed } from '@angular/core/testing';
import { adminGuard } from './admin.guard';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AccountService } from '../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';

describe('AdminGuard', () => {
  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = {} as RouterStateSnapshot;
  let mockAccountService = {
    roles: jest.fn().mockReturnValue([]),
  };
  let mockToast = {
    error: jest.fn(),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClientTesting(),
        provideHttpClient(),
        { provide: AccountService, useValue: mockAccountService },
        { provide: HotToastService, useValue: mockToast },
      ],
    });
    mockAccountService.roles.mockReset();
    mockToast.error.mockReset();
  });

  it('returns false for non logged or non admin user', () => {
    mockAccountService.roles.mockReturnValue([]);
    let result;
    TestBed.runInInjectionContext(() => {
      result = adminGuard(mockRoute, mockState);
    });
    expect(result).toBe(false);
    expect(mockToast.error).toHaveBeenCalled();
  });

  it('returns true for admin user', () => {
    mockAccountService.roles.mockReturnValue(['Admin', 'Moderator']);
    let result;
    TestBed.runInInjectionContext(() => {
      result = adminGuard(mockRoute, mockState);
    });
    expect(result).toBe(true);
    expect(mockToast.error).not.toHaveBeenCalled();
  });
});
