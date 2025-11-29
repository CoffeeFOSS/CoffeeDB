import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthDirective } from './auth.directive';
import { AccountService } from '../services/account.service';
import { By } from '@angular/platform-browser';
import { signal } from '@angular/core';
import { User } from '../models/user';

@Component({
  template: `
    <div *appAuth>Protected Content</div>
  `,
  imports: [AuthDirective],
})
class TestComponent {}

class MockAccountService {
  currentUser = signal<User | null>(null);
}

describe('AuthDirective', () => {
  let fixture: ComponentFixture<any>;
  let accountService: MockAccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TestComponent],
      providers: [{ provide: AccountService, useClass: MockAccountService }],
    }).createComponent(TestComponent);

    fixture = TestBed.createComponent(TestComponent);
    accountService = TestBed.inject(AccountService) as MockAccountService;
  });

  it('should NOT render content when user is not logged in', () => {
    accountService.currentUser.set(null);
    fixture.detectChanges();
    const element = fixture.debugElement.query(By.css('div'));
    expect(element).toBeNull();
  });

  it('should render content when user is logged in', () => {
    accountService.currentUser.set({ id: 1, username: 'Alice', token: 'test' });
    fixture.detectChanges();
    const element = fixture.debugElement.query(By.css('div'));
    expect(element).not.toBeNull();
    expect(element.nativeElement.textContent).toBe('Protected Content');
  });
});
