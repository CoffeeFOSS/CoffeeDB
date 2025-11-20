import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoasterFrameComponent } from './roaster-frame.component';

describe('RoasterFrameComponent', () => {
  let component: RoasterFrameComponent;
  let fixture: ComponentFixture<RoasterFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoasterFrameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoasterFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
