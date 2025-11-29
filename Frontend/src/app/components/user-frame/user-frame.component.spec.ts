import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserFrameComponent } from './user-frame.component';

describe('UserFrameComponent', () => {
  let component: UserFrameComponent;
  let fixture: ComponentFixture<UserFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserFrameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
