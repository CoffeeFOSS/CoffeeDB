import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadingIndicatorComponent } from './loading-indicator.component';
import { LoadingService } from '../../services/loading.service';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

describe('LoadingIndicatorComponent', () => {
  let component: LoadingIndicatorComponent;
  let fixture: ComponentFixture<LoadingIndicatorComponent>;
  let loadingService: LoadingService;
  let bar: DebugElement;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [LoadingIndicatorComponent],
      providers: [LoadingService],
    }).compileComponents();

    loadingService = TestBed.inject(LoadingService);
    fixture = TestBed.createComponent(LoadingIndicatorComponent);
    component = fixture.componentInstance;
  });

  it('creates component', () => {
    expect(component).toBeTruthy;
  });

  it('set initial data', () => {
    expect(component.progress()).toBe(0);
    expect(component.visible()).toBe(false);
    fixture.detectChanges();
    // completeProgress runs first, which sets progress to 1
    expect(component.progress()).toBe(1);
    expect(component.visible()).toBe(false);
  });

  describe('loading functionality', () => {
    beforeEach(() => {
      bar = fixture.debugElement.query(
        By.css('[data-testid="loading-indicator-bar"]'),
      );
    });

    it('renders loading indicator bar', () => {
      expect(bar).toBeTruthy();
    });

    it('should start then stop progress with loading state', () => {
      jest.useFakeTimers();

      loadingService.busy();
      fixture.detectChanges();
      expect(component.visible()).toBe(true);
      expect(bar.nativeElement.style.opacity).toBe('1');

      loadingService.idle();
      fixture.detectChanges();
      expect(component.visible()).toBe(true);

      jest.advanceTimersByTime(200);
      fixture.detectChanges();
      expect(component.visible()).toBe(false);
      expect(bar.nativeElement.style.opacity).toBe('0');
    });
  });
});
