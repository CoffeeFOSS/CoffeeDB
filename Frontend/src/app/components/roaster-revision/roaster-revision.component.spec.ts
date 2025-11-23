import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoasterRevisionComponent } from './roaster-revision.component';

describe('RoasterRevisionComponent', () => {
  let component: RoasterRevisionComponent;
  let fixture: ComponentFixture<RoasterRevisionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoasterRevisionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoasterRevisionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
