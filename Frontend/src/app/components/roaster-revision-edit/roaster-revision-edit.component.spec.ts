import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoasterRevisionEditComponent } from './roaster-revision-edit.component';

describe('RoasterRevisionEditComponent', () => {
  let component: RoasterRevisionEditComponent;
  let fixture: ComponentFixture<RoasterRevisionEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoasterRevisionEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoasterRevisionEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
